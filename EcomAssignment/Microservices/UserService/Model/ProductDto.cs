namespace UserService.Model
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Size { get; set; }
        public string? Design { get; set; }
        public int Quantity { get; set; }
    }
}
