namespace Nesting_4
{
	public interface IConfigurationIO
	{
        Configuration ReadAllData(string fileName);

        void WriteAllData(ref Configuration configuration, string fileName);
	}
}