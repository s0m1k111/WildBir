namespace WildBir.Models
{
    public class ProductModel(string Id, string Image, string Price, string Name)
    {
        public string Id { get; set; } = Id;
        public string Image { get; set; } = Image;
        public string Price { get; set; } = Price;
        public string Name { get; set; } = Name;
    }
}
