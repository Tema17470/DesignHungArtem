namespace Application.Abstractions;
public interface ISensorNormalizer
{
    string CanonicalUnit { get; }
    double Normalize(double raw);
}
