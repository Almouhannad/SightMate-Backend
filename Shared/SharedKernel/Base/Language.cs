namespace SharedKernel.Base;

public class Language
{
    public String Name { get; init; }
    public String Code { get; init; }

    public Language(String name, String code)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code cannot be null or whitespace.", nameof(code));
        }

        Name = name;
        Code = code;
    }
}
