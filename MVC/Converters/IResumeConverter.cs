namespace Resume.Converters
{
	public interface IResumeConverter
	{
		void WriteToStream(Resume.Models.Resume resume, System.IO.Stream outputStream);
	}
}
