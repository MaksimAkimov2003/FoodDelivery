﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Food_Delivery.Common;
using Food_Delivery.Common.db;
using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Food_Delivery.Services.Users;

public class UsersService : IUsersService
{
    private readonly AddressDbContext _addressDbContext;
    private readonly ApplicationDbContext _applicationDbContext;

    public UsersService(ApplicationDbContext applicationDbContext, AddressDbContext addressDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _addressDbContext = addressDbContext;
    }

    public async Task<TokenResponse> RegisterUser(UserRegisterModel userRegisterModel)
    {
        CheckPassword(userRegisterModel.Password);
        userRegisterModel.Email = NormalizeAttribute(userRegisterModel.Email);

        await UniqueCheck(userRegisterModel);

        byte[] salt;
        RandomNumberGenerator.Create().GetBytes(salt = new byte[16]);
        var pbkdf2 = new Rfc2898DeriveBytes(userRegisterModel.Password, salt, 100000);
        var hash = pbkdf2.GetBytes(20);
        var hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        var savedPasswordHash = Convert.ToBase64String(hashBytes);

        CheckGender(userRegisterModel.Gender);
        CheckBirthDate(userRegisterModel.BirthDate);
        CheckAddress(houseGuid: userRegisterModel.Address);

        await _applicationDbContext.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = userRegisterModel.FullName,
            BirthDate = userRegisterModel.BirthDate,
            Address = userRegisterModel.Address,
            Email = userRegisterModel.Email,
            Gender = userRegisterModel.Gender,
            Password = savedPasswordHash,
            PhoneNumber = userRegisterModel.PhoneNumber,
        });
        await _applicationDbContext.SaveChangesAsync();

        var credentials = new LoginCredentials
        {
            Email = userRegisterModel.Email,
            Password = userRegisterModel.Password
        };

        return await LoginUser(credentials);
    }

    public async Task<TokenResponse> LoginUser(LoginCredentials credentials)
    {
        credentials.Email = NormalizeAttribute(credentials.Email);

        var identity = await GetIdentity(credentials.Email, credentials.Password);

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: JwtConfigurations.Issuer,
            audience: JwtConfigurations.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.AddMinutes(JwtConfigurations.Lifetime),
            signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var result = new TokenResponse()
        {
            Token = encodeJwt
        };

        return result;
    }

    public async Task Logout(string token)
    {
        var alreadyExistsToken = await _applicationDbContext.Tokens.FirstOrDefaultAsync(x => x.InvalidToken == token);

        if (alreadyExistsToken == null)
        {
            var handler = new JwtSecurityTokenHandler();
            var expiredDate = handler.ReadJwtToken(token).ValidTo;
            _applicationDbContext.Tokens.Add(new Token { InvalidToken = token, ExpiredDate = expiredDate });
            await _applicationDbContext.SaveChangesAsync();
        }
        else
        {
            var ex = new Exception("Token is already invalid");
            throw ex;
        }
    }

    public async Task<UserDto> GetUserProfile(Guid userId)
    {
        var userEntity = await _applicationDbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity != null)
            return new UserDto
            {
                Address = userEntity.Address,
                BirthDate = userEntity.BirthDate,
                Email = userEntity.Email,
                FullName = userEntity.FullName,
                Gender = userEntity.Gender,
                Id = userEntity.Id,
                PhoneNumber = userEntity.PhoneNumber
            };

        var ex = new Exception("User not exists");
        throw ex;
    }

    public async Task EditUserProfile(Guid userId, UserEditModel userEditModel)
    {
        var userEntity = await _applicationDbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (userEntity == null)
        {
            var ex = new Exception("User not exists");
            throw ex;
        }

        CheckAddress(userEditModel.Address);
        CheckGender(userEditModel.Gender);
        CheckBirthDate(userEditModel.BirthDate);

        userEntity.FullName = userEditModel.FullName;
        userEntity.BirthDate = userEditModel.BirthDate;
        userEntity.Address = userEditModel.Address;
        userEntity.Gender = userEditModel.Gender;
        userEntity.PhoneNumber = userEditModel.PhoneNumber;

        await _applicationDbContext.SaveChangesAsync();
    }

    private async Task<ClaimsIdentity> GetIdentity(string email, string password)
    {
        var userEntity = await _applicationDbContext
            .Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (userEntity == null)
        {
            var ex = new Exception("User not exists");
            throw ex;
        }

        if (!CheckHashPassword(userEntity.Password, password))
        {
            var ex = new Exception("Wrong password");
            throw ex;
        }

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, userEntity.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity
        (
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        return claimsIdentity;
    }

    private static bool CheckHashPassword(string savedPasswordHash, string password)
    {
        var hashBytes = Convert.FromBase64String(savedPasswordHash);
        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
        var hash = pbkdf2.GetBytes(20);
        for (var i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
                return false;
        return true;
    }

    private static string NormalizeAttribute(string value)
    {
        return value.ToLower().TrimEnd();
    }

    private async Task UniqueCheck(UserRegisterModel userRegisterModel)
    {
        var email = await _applicationDbContext
            .Users
            .Where(x => userRegisterModel.Email == x.Email)
            .FirstOrDefaultAsync();

        if (email != null)
        {
            var ex = new Exception($"Account with email '{userRegisterModel.Email}' already exists");
            throw ex;
        }
    }

    private void CheckPassword(String password)
    {
        if (password.Length < 8)
        {
            throw new Exception("Password lenght should be >= 8 symbols");
        }

        if (!password.Any(Char.IsDigit))
        {
            throw new Exception("Password should contain digits");
        }
    }

    private void CheckAddress(Guid houseGuid)
    {
        var objectGuidParam = new NpgsqlParameter("objectGuid", houseGuid);

        var houseEntity = _addressDbContext.Set<HouseEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_houses WHERE objectguid=@objectGuid", objectGuidParam)
            .FirstOrDefault();

        if (houseEntity == null)
        {
            throw new Exception("House guid expected in address field");
        }
    }

    private static void CheckGender(string gender)
    {
        if (gender == Gender.Male.ToString() || gender == Gender.Female.ToString()) return;

        var ex = new Exception($"Possible Gender values: {Gender.Male.ToString()}, {Gender.Female.ToString()}");
        throw ex;
    }

    private static void CheckBirthDate(DateTime? birthDate)
    {
        if (birthDate == null || birthDate <= DateTime.Now) return;

        var ex = new Exception("Birth date can't be later than today");
        throw ex;
    }
}