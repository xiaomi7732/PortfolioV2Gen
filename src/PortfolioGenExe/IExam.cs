namespace PortfolioGenExe;

internal interface IExam
{
    (bool Pass, string? Reason) Execute(DataMeta data);
}