using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Repositories;

public class SupportedManufacturersStaticRepository : ISupportedManufacturersRepository
{
    private static readonly string[] SupportedManufacturers =
    [
        "BRIDGESTONE", "CONTINENTAL", "DUNLOP", "GOODYEAR", "MICHELIN", "NOKIAN TYRES", "PIRELLI", "YOKOHAMA", "FALKEN",
        "FIRESTONE", "FULDA", "GITI", "GT RADIAL", "HANKOOK", "KLEBER", "KUMHO", "NEXEN", "TOYO", "VREDESTEIN",
        "UNIROYAL", "AVON", "BARUM", "BFGOODRICH", "DEBICA", "GRIPMAX", "HEIDENAU", "IMPERIAL", "LAUFENN", "MATADOR",
        "MAXXIS", "NANKANG", "RIKEN", "ROYAL BLACK", "SAVA", "SEMPERIT", "TRACMAX", "WINDFORCE", "ZMAX", "ACHILLES",
        "ALTENZO", "ANTARES", "APLUS", "APOLLO", "ARIVO", "ATLAS", "AUSTONE", "CEAT", "COMFORSER", "COOPER TIRES",
        "CST", "DAVANTI", "DAYTON", "DELINTE", "DEXTERO", "DIPLOMAT", "DURATURN", "EVERGREEN", "FEDERAL", "FIREMAX",
        "FORTUNA", "FORTUNE", "FRONWAY", "GENERAL TIRE", "GISLAVED", "GOLDLINE", "GOODRIDE", "GREENTRAC", "GREMAX",
        "GRENLANDER", "HIFLY", "HILO", "INFINITY", "INTERSTATE", "KELLY", "KENDA", "KINGSTAR", "KORMORAN", "LANDSAIL",
        "LASSA", "LEAO", "LINGLONG", "MABOR", "MAZZINI", "MEMBAT", "MILESTONE", "MINERVA", "MIRAGE", "MOMO", "NORDEXX",
        "NOVEX", "ORIUM", "OTANI", "OVATION", "PACE", "PAXARO", "PETLAS", "PLATIN", "POINTS", "POWERTRAC", "RADAR",
        "ROADMARCH", "ROADSTONE", "ROADX", "ROCKBLADE", "ROSAVA", "ROTALLA", "ROTEX", "RUNWAY", "SAILUN", "SEBRING",
        "SEIBERLING", "SILVERSTONE", "SPORTIVA", "STARMAXX", "SUMITOMO", "SUNFULL", "SUNNY", "SUPERIA", "SYRON",
        "TAURUS", "TIGAR", "TORQUE", "TOURADOR", "TRIANGLE", "TRISTAR", "TYFOON", "UNIGRIP", "VIATTI", "VIKING",
        "WANDA", "WANLI", "WATERFALL", "WESTLAKE", "WINRUN", "ZEETEX", "ZETA"
    ];

    public Task<IEnumerable<SupportedManufacturerDto>> GetSupportedManufacturers() =>
        Task.FromResult(SupportedManufacturers
            .Select(x => new SupportedManufacturerDto(x))
        );
}