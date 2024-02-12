namespace Excalibur.Input;

public interface IInputReader<out T>
{
    IEnumerable<T> ReadItems(string fileName);
}
