using NTS.Compatibility.EMS.Abstractions;

namespace NTS.Compatibility.EMS.Entities.Results;

public class EmsResult : EmsDomainBase<EmsResultException>, IEmsResultState
{
    [Newtonsoft.Json.JsonConstructor]
    private EmsResult() {}
    internal EmsResult(ResultType type, string code = null) : base(default)
    {
        this.Code = code;
        this.Type = type;
    }

    public bool IsNotQualified => this.Type != ResultType.Successful;
    public string Code { get; private set; }
    public ResultType Type { get; private set; } = ResultType.Successful;
    public string TypeCode
    {
        get
        {
            switch (this.Type)
            {
                case ResultType.Resigned: return "RET";
                case ResultType.FailedToQualify: return "FTQ";
                case ResultType.Disqualified: return "DSQ";
                case ResultType.Successful: return "R";
                case ResultType.Invalid:
                default: throw EmsHelper.Create<EmsResultException>("Invalid result type");
            }
        }
    }

    public override string ToString()
    {
        if (this.Type == ResultType.Successful)
        {
            return string.Empty;
        }
        var typeString = string.Empty;
        

        return $"{typeString.ToUpper()} {TypeCode}";
    }
}

public enum ResultType
{
    Invalid = 0,
    Resigned = 1,
    FailedToQualify = 2,
    Disqualified = 3,
    Successful = 4
}
