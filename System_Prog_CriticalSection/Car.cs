namespace System_Prog_CriticalSection;

public class Car
{
    public string Model { get; set; }   
    public string Vendor { get; set; }
    public int Year { get; set; }
    public string ImagePath { get; set; }

    public Car()
    {
        
    }

    public Car(string model, string vendor, int year, string imagePath)
    {
        Model = model;
        Vendor = vendor;
        Year = year;
        ImagePath = imagePath;
    }
}
