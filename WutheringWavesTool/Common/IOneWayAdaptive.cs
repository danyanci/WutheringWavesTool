namespace WutheringWavesTool.Common;

public interface IOneWayAdaptive<Orgin, Forward>
{
    public Forward Convert(Orgin orgin);
}
