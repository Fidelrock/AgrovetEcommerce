using Agrovet.Domain.Common;

namespace Agrovet.Domain.Entities;

public class ProductMedia : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }

    public string Url { get; private set; }
    public string MediaType { get; private set; } // image | video | model3d
    public int DisplayOrder { get; private set; }

    private ProductMedia() { }

    public ProductMedia(Guid productId, string url, string mediaType, int displayOrder)
    {
        ProductId = productId;
        Url = url;
        MediaType = mediaType;
        DisplayOrder = displayOrder;
    }
}
