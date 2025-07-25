using System.ComponentModel.DataAnnotations;

public class ProductReqDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int Quantity { get; set; }

    [Required]
    public int ProductTypeId { get; set; }
    public bool IsDeleted { get; set; }

}
