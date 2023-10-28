using System.ComponentModel;

namespace Food_Delivery.Models.Dto;

public enum GarAddressLevel
{
    [Description("Субъект РФ")] Region,

    [Description("Административный район")]
    AdministrativeArea,

    [Description("Районный центр")] MunicipalArea,

    [Description("Районный центр")] RuralUrbanSettlement,

    [Description("Город")] City,

    [Description("Населенный пункт")] Locality,

    [Description("Элемент планировочной структуры")]
    ElementOfPlanningStructure,

    [Description("Элемент улично-дорожной сети")]
    ElementOfRoadNetwork,

    [Description("Земля")] Land,

    [Description("Здание(строение)")] Building,

    [Description("Комната")] Room,

    [Description("Комната в комнате")] RoomInRooms,

    [Description("Автономный регион")] AutonomousRegionLevel,

    [Description("Административный район")]
    IntracityLevel,

    [Description("Элемент планировочной сети")]
    AdditionalTerritoriesLevel,

    [Description("Уровень объектов на дополнительных территориях (устаревшее)")]
    LevelOfObjectsInAdditionalTerritories,

    [Description("Гараж")] CarPlace
}