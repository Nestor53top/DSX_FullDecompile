namespace Squirrel.SimpleSplat;

public interface IModeDetector
{
	bool? InUnitTestRunner();

	bool? InDesignMode();
}
