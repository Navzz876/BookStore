using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$" , ErrorMessage = "Invalid name format, Please enter the correct name")]
        [MaxLength(100)]
        public string? Name { get; set; }
        [Range(0,100 , ErrorMessage = "Display Order cannot be less than 0, Please enter the correct Display Order")]
        public int DisplayOrder { get; set; }

    }
}