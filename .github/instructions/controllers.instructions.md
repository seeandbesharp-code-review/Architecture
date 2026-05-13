# Controllers Reference

**Location**: `ApiProject2/ApiProject/Controllers/`

## Structure Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class XxxController : ControllerBase
{
    private readonly IXxxService _service;
    public XxxController(IXxxService service) => _service = service;
    // No business logic here — delegate everything to the service
}
```

---

## AuthController
**Route**: `api/auth` | **Service**: `IAuthService`

| Method | Endpoint | Body / Params | Returns |
|--------|----------|---------------|---------|
| POST | `/api/auth/login` | `AuthorDto.LoginDto` | JWT token |
| POST | `/api/auth/register` | `AuthorDto.RegisterDto` | Created user |
| POST | `/api/auth/refresh` | Refresh token | New JWT token |

---

## GiftsController
**Route**: `api/gifts` | **Service**: `IGiftsService` | **DTO**: `GiftDto`

| Method | Endpoint | Params | Returns |
|--------|----------|--------|---------|
| GET | `/api/gifts` | — | `IEnumerable<GiftDto>` |
| GET | `/api/gifts/{id}` | `id` (path) | `GiftDto` |
| GET | `/api/gifts/sortByName` | `name` (query) | Filtered gifts |
| GET | `/api/gifts/sortByBuyers` | — | Gifts with buyer counts |
| GET | `/api/gifts/sortByDonorName` | `name` (query) | Gifts by donor |
| GET | `/api/gifts/OrderByPrice` | — | Gifts sorted by price |
| GET | `/api/gifts/OrderByCategory` | `id` (query) | Gifts in category |
| POST | `/api/gifts` | `GiftDto.AddGiftDto` (body) | Created gift |
| PUT | `/api/gifts/{id}` | `GiftDto.UpdateGiftDto` (body) | Updated gift |
| DELETE | `/api/gifts/{id}` | `id` (path) | Updated gift list |

**`AddGiftDto` fields**: `Name`, `Price`, `Description`, `DonorId`, `CategoryId`, `ImageUrl`  
**`UpdateGiftDto` fields**: same as above (all optional except `Id`)

---

## CartController
**Route**: `api/cart` | **Service**: `ICartService` | **DTO**: `CartDto`

| Method | Endpoint | Params | Returns |
|--------|----------|--------|---------|
| GET | `/api/cart/{userId}` | `userId` (path) | `CartDto.CartModelDto` |
| GET | `/api/cart/unpaid/{userId}` | `userId` (path) | Draft cart |
| GET | `/api/cart/paid/{userId}` | `userId` (path) | List of paid carts |
| POST | `/api/cart/addGift` | `AddCartItemDto` (body) | Updated cart |
| POST | `/api/cart/plusOne` | `AddCartItemDto` (body) | Updated cart |
| POST | `/api/cart/minusOne` | `AddCartItemDto` (body) | Updated cart |
| PUT | `/api/cart/updateGift` | `UpdateCartItemDto` (body) | Updated cart |
| PUT | `/api/cart/purchase` | `userId` (query) | Purchased cart |
| DELETE | `/api/cart/deleteGift` | `userId`, `giftId` (query) | Updated cart |

**`AddCartItemDto` fields**: `UserId`, `GiftId`, `Quantity`  
**`UpdateCartItemDto` fields**: `CartItemId`, `Quantity`

---

## CategoryController
**Route**: `api/category` | **Service**: `ICategoryService` | **DTO**: `CategoryDto`

| Method | Endpoint | Params | Returns |
|--------|----------|--------|---------|
| GET | `/api/category` | — | `IEnumerable<CategoryDto>` |
| GET | `/api/category/{id}` | `id` (path) | `CategoryDto` |
| POST | `/api/category` | `CategoryDto` (body) | Updated category list |
| PUT | `/api/category/{id}` | `id` (path), `CategoryDto` (body) | Updated category |
| DELETE | `/api/category/{id}` | `id` (path) | Updated category list |

**`CategoryDto` fields**: `Name`

---

## DonorsController
**Route**: `api/donors` | **Service**: `IDonorService` | **DTO**: `DonorDto`

| Method | Endpoint | Params | Returns |
|--------|----------|--------|---------|
| GET | `/api/donors` | — | `IEnumerable<DonorDto>` |
| GET | `/api/donors/{id}` | `id` (path) | `DonorDto` |
| GET | `/api/donors/searchDonorByFullName` | `name` (query) | Matching donors |
| POST | `/api/donors` | `DonorDto.AddDonorDto` (body) | Created donor |
| PUT | `/api/donors/{id}` | `DonorDto.UpdateDonorDto` (body) | Updated donor |
| DELETE | `/api/donors/{id}` | `id` (path) | Success message |

**`AddDonorDto` fields**: `FirstName`, `LastName`, `Phone`, `Email`  
**`UpdateDonorDto` fields**: same + `Id`

---

## SalesController
**Route**: `api/sales` | **Service**: `ISalesService` | **DTO**: `SalesDto`

| Method | Endpoint | Returns |
|--------|----------|---------|
| GET | `/api/sales/gift-resumes` | `List<SalesDto.GiftResume>` — summary per gift |
| GET | `/api/sales/gift-buyers` | `List<SalesDto.GiftsWithBuyers>` — gifts + buyer details |
| GET | `/api/sales/gifts-by-price` | Gifts sorted by price descending |
| GET | `/api/sales/gifts-by-quantity` | Gifts sorted by quantity sold descending |

**Read-only controller** — no POST/PUT/DELETE. Data is derived from purchases.

---

## LotteryController
**Route**: `api/lottery` | **Service**: `ILotteryService` | **DTO**: `RaffleDto`

| Method | Endpoint | Params | Returns |
|--------|----------|--------|---------|
| POST | `/api/lottery/{giftId}` | `giftId` (path) | `RaffleDto.LotteryResult` |

**Behavior**: Draws a winner from all purchasers of the gift, weighted by ticket quantity. Can only run once per gift (gift is marked as raffled after draw).

**`RaffleDto.LotteryResult` fields**: `WinnerId`, `WinnerName`, `GiftId`, `GiftName`
