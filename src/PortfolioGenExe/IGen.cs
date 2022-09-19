namespace PortfolioGenExe;
internal interface IGen
{
    bool CanGen(DataMeta data);
    string Generate(DataMeta data);

}
