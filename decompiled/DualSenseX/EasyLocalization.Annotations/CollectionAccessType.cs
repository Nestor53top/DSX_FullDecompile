using System;

namespace EasyLocalization.Annotations;

[Flags]
public enum CollectionAccessType
{
	None = 0,
	Read = 1,
	ModifyExistingContent = 2,
	UpdatedContent = 6
}
