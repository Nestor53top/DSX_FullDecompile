using System.Collections.Generic;

namespace Microsoft.AppCenter.Crashes;

public delegate IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsCallback(ErrorReport report);
