using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Utils;

public interface IDeviceInformationHelper
{
	Task<Microsoft.AppCenter.Ingestion.Models.Device> GetDeviceInformationAsync();

	Microsoft.AppCenter.Ingestion.Models.Device GetDeviceInformation();
}
