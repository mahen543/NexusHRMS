using HRMS_HikeFlow.NexusProperties;

namespace NexusHRMS.HikeFlowWeb.Services
{
    public interface ITokenGeneratorService
    {
        string GenerateToken(UserAuthModel user);
    }
}