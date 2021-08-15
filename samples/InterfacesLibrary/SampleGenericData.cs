namespace InterfacesLibrary
{
    public class SampleGenericData<TInner>
    {
        public string SampleInfo { get; set; }
        public TInner SampleComplexInfo { get; set; }
    }
}