namespace GrainInterfaces
{
    using Orleans;
    using Orleans.CodeGeneration;
    using System.Threading.Tasks;

    [Version(1)]
    public interface IRequestContextTestGrain : IGrainWithIntegerKey
    {
        Task DisplayRequestContext();
    }
}