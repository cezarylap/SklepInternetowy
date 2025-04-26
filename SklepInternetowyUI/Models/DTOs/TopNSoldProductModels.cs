namespace ProductSklepInternetowyUI.Models.DTOs;

public record TopNSoldProductModel(string ProductName, string AuthorName, int TotalUnitSold);
public record TopNSoldProductsVm(DateTime StartDate, DateTime EndDate, IEnumerable<TopNSoldProductModel> TopNSoldProducts);