namespace Squirrel.SimpleSplat;

internal interface IModeDetector
{
	bool? InUnitTestRunner();

	bool? InDesignMode();
}
