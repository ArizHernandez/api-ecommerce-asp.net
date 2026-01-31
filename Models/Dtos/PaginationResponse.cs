using System;

namespace apiEcommerce.Models.Dtos;

public class PaginationResponse<T>
{
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
  public ICollection<T> Data { get; set; } = new List<T>();
}
