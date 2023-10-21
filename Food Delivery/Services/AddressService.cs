﻿using System.Text.RegularExpressions;
using Food_Delivery.Common.db;
using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Food_Delivery.Services;

public class AddressService : IAddressService
{
    private readonly AddressDbContext _context;

    public AddressService(AddressDbContext context)
    {
        _context = context;
    }

    public List<SearchAddressDto> GetAddressChain(Guid objectGuid)
    {
        var objectInfo = GetObjectInfo(objectGuid: objectGuid);

        var path = GetAddressPath(objectInfo.Id);

        // Айдишники родительских объектов
        var parentsIdList = ParseAddressPath(path);
        parentsIdList.RemoveAt(parentsIdList.Count - 1);

        if (objectInfo is ObjectInfo.AddressObject info)
        {
            return FindAddressChain
            (
                addressObject: info,
                parentsIdList: parentsIdList
            );
        }

        if (objectInfo is ObjectInfo.House house)
        {
            return FindAddressChain
            (
                house: house,
                parentsIdList: parentsIdList
            );
        }


        throw new NotImplementedException();
    }

    private ObjectInfo GetObjectInfo(Guid objectGuid)
    {
        var objectGuidParam = new NpgsqlParameter("objectGuid", objectGuid);

        // Проверяем, адрессный объект ли это или строение
        var addressObject = _context.Set<AddressObjectEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_addr_obj WHERE objectguid=@objectGuid", objectGuidParam)
            .FirstOrDefault();

        if (addressObject != null)
        {
            // Если это адрессный объект
            return new ObjectInfo.AddressObject
            {
                Id = addressObject.objectid,
                Guid = addressObject.objectguid,
                Level = addressObject.level,
                Name = addressObject.name,
                TypeName = addressObject.typename
            };
        }

        // Если это строение
        var buildingObject = _context.Set<HouseEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_houses WHERE objectguid=@objectGuid", objectGuidParam)
            .FirstOrDefault();

        return new ObjectInfo.House
        {
            Id = buildingObject!.objectid,
            Guid = buildingObject.objectguid,
            HouseNum = buildingObject.housenum,
            AddNum1 = buildingObject.addnum1,
            AddNum2 = buildingObject.addnum2
        };
    }

    private string GetAddressPath(Int64 id)
    {
        var objectIdParam = new NpgsqlParameter("objectid", id);

        var hierarchy = _context.Set<AdministrativeHierarchyEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_adm_hierarchy WHERE objectid=@objectid", objectIdParam)
            .FirstOrDefault();

        return hierarchy!.path;
    }

    private List<Int64> ParseAddressPath(string path)
    {
        return path.Split('.').Select(long.Parse).ToList();
    }

    private GarAddressLevel FindLevelByLevelNumber(int number)
    {
        foreach (GarAddressLevel level in Enum.GetValues(typeof(GarAddressLevel)))
        {
            if ((int)level == number)
            {
                return level;
            }
        }

        throw new KeyNotFoundException();
    }

    private List<SearchAddressDto> FindAddressChain(
        ObjectInfo.AddressObject addressObject,
        List<Int64> parentsIdList
    )
    {
        var resultList = FetchParentObjects(parentsIdList: parentsIdList);

        SearchAddressDto addressInfo = new SearchAddressDto
        {
            ObjectGuid = addressObject.Guid,
            ObjectId = addressObject.Id,
            ObjectLevel = FindLevelByLevelNumber(int.Parse(addressObject.Level) - 1),
            Text = addressObject.TypeName + " " + addressObject.Name
        };

        resultList.Add(addressInfo);

        if (resultList.IsNullOrEmpty())
        {
            throw new ArgumentNullException();
        }

        return resultList;
    }


    private List<SearchAddressDto> FindAddressChain(
        ObjectInfo.House house,
        List<Int64> parentsIdList
    )
    {
        var resultList = FetchParentObjects(parentsIdList: parentsIdList);

        var text = house.HouseNum;

        if ((house.AddNum1 != null) && (Regex.IsMatch(house.AddNum1, "^[0-9]+$")))
        {
            text += " стр. " + house.AddNum1;
        }

        if ((house.AddNum2 != null) && (Regex.IsMatch(house.AddNum2, "^[0-9]+$")))
        {
            text += " соор. " + house.AddNum2;
        }

        SearchAddressDto houseInfo = new SearchAddressDto
        {
            ObjectGuid = house.Guid,
            ObjectId = house.Id,
            ObjectLevel = GarAddressLevel.Building,
            Text = text
        };

        resultList.Add(houseInfo);

        if (resultList.IsNullOrEmpty())
        {
            throw new ArgumentNullException();
        }

        return resultList;
    }

    private List<SearchAddressDto> FetchParentObjects
    (
        List<Int64> parentsIdList
    )
    {
        var resultList = new List<SearchAddressDto>();

        // Ищем информацию о родительских объектах(они всегда будут адресообразующими)
        foreach (var objectId in parentsIdList)
        {
            var objectIdParam = new NpgsqlParameter("objectid", objectId);

            var addressObjectEntity = _context.Set<AddressObjectEntity>()
                .FromSqlRaw("SELECT * FROM fias.as_addr_obj WHERE objectid=@objectid", objectIdParam)
                .FirstOrDefault();

            SearchAddressDto searchAddressDto = new SearchAddressDto
            {
                ObjectId = addressObjectEntity!.id,
                ObjectGuid = addressObjectEntity.objectguid,
                ObjectLevel = FindLevelByLevelNumber(int.Parse(addressObjectEntity.level) - 1),
                Text = addressObjectEntity.typename + " " + addressObjectEntity.name
            };

            resultList.Add(searchAddressDto);
        }

        return resultList;
    }
}