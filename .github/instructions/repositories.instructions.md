# Repositories Reference

**Location**: `ApiProject2/ApiProject/Repositories/`  
**Interfaces**: `Repositories/Interface/IXxxRepository.cs`  
**Implementations**: `Repositories/Implement/XxxRepository.cs`

## Structure Pattern

```csharp
// Interface
public interface IXxxRepository
{
    Task<XxxModel> GetById(int id);
}

// Implementation
public class XxxRepository : IXxxRepository
{
    private readonly ProjectContext _context;
    private readonly ILogger<XxxRepository> _logger;

    public XxxRepository(ProjectContext context, ILogger<XxxRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<XxxModel> GetById(int id)
    {
        try
        {
            _logger.LogInformation("...");
            return await _context.Xxx.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "...");
            throw;
        }
    }
}
```

**Rules**:
- All methods are `async`/`await`
- Use `_context.SaveChangesAsync()` after every write operation
- Use `.Include()` / `.ThenInclude()` for relations — no lazy loading
- Log before and on error using `_logger`
- Error messages in **Hebrew**

---

## IAuthRepository / AuthRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetByEmail | `Task<UserModel> GetByEmail(string email)` | Find user by email (for login) |
| AddUser | `Task<UserModel> AddUser(UserModel user)` | Insert new user record |

**Relations**: None (UserModel is standalone)

---

## ICartRepository / CartRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| CreateCart | `Task<CartModel> CreateCart(CartModel cart)` | Insert new cart |
| GetCartByUserId | `Task<CartModel> GetCartByUserId(int userId)` | Get active cart with items |
| GetUnpaidCartByUserId | `Task<CartModel> GetUnpaidCartByUserId(int userId)` | Draft (status = unpaid) cart |
| GetPaidCartByUserId | `Task<CartModel> GetPaidCartByUserId(int userId)` | All completed carts |
| GetOrCreateDraftCartByUserId | `Task<CartModel> GetOrCreateDraftCartByUserId(int userId)` | Get draft or create one if none exists |
| AddGiftToCart | `Task<CartItemModel> AddGiftToCart(CartItemModel cartItem)` | Add new CartItem row |
| UpdateGiftToCart | `Task<CartItemModel> UpdateGiftToCart(CartItemModel cartItem)` | Update existing CartItem |
| DeleteGiftFromCart | `Task<CartModel> DeleteGiftFromCart(int userId, int giftId)` | Remove CartItem from cart |
| GetCartItemById | `Task<CartItemModel> GetCartItemById(int cartItemId)` | Fetch single CartItem |
| GetAllPaidCartDtosByUserId | `Task<List<CartDto.CartModelDto>> GetAllPaidCartDtosByUserId(int userId)` | Projection to DTOs |
| PurchaseCart | `Task<CartModel> PurchaseCart(CartModel cart)` | Set cart status = paid |
| UpdateCart | `Task<CartModel> UpdateCart(CartModel cart)` | Save changes to cart record |
| CheckIfUserHaveThisGift | `Task<CartItemModel> CheckIfUserHaveThisGift(int cartId, int giftId)` | Returns item or null |
| CheckGiftIsRaffled | `Task<bool> CheckGiftIsRaffled(int giftId)` | Returns true if gift was already raffled |

**Eager load**: Cart → `.Include(c => c.CartItems).ThenInclude(ci => ci.Gift)`

---

## ICategoryRepository / CategoryRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetAllCategories | `Task<IEnumerable<CategoryModel>> GetAllCategories()` | All categories |
| GetCategoryById | `Task<CategoryModel> GetCategoryById(int id)` | Single category |
| AddCategory | `Task<IEnumerable<CategoryModel>> AddCategory(CategoryModel category)` | Insert, return updated list |
| UpdateCategoryR | `Task<CategoryModel> UpdateCategoryR(CategoryModel category, int id)` | Update, return updated entity |
| DeleteCategory | `Task<IEnumerable<CategoryModel>> DeleteCategory(int id)` | Delete, return updated list |

**Note**: Add/Delete return the full list (not just the affected item) — controllers use this to refresh UI state.

---

## IDonorRepository / DonorRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetAllDonors | `Task<IEnumerable<DonorModel>> GetAllDonors()` | All donors |
| GetDonorById | `Task<DonorModel> GetDonorById(int id)` | Single donor |
| AddDonor | `Task<string> AddDonor(DonorModel donor)` | Insert, return Hebrew success message |
| UpdateDonor | `Task<DonorModel> UpdateDonor(DonorModel donor, int id)` | Update, return updated entity |
| DeleteDonor | `Task<string> DeleteDonor(DonorModel donor)` | Soft or hard delete, return Hebrew message |
| SearchByNameAsync | `Task<IEnumerable<DonorModel>> SearchByNameAsync(string searchTerm)` | Partial name search (`Contains`) |

---

## IGiftRepository / GiftRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetAllGifts | `Task<IEnumerable<GiftModel>> GetAllGifts()` | All gifts with relations |
| GetGiftById | `Task<GiftModel> GetGiftById(int id)` | Single gift with Donor + Category |
| AddGift | `Task<string> AddGift(GiftModel gift)` | Insert, return Hebrew success message |
| CheckDonorExist | `Task<bool> CheckDonorExist(int id)` | Validation before add/update |
| CheckNameExist | `Task<bool> CheckNameExist(string name)` | Uniqueness check |
| CheckCategoryExist | `Task<bool> CheckCategoryExist(int categoryId)` | Validation before add/update |
| UpdateGift | `Task<GiftModel> UpdateGift(GiftModel gift, int id)` | Update, return updated entity |
| DeleteGift | `Task<IEnumerable<GiftModel>> DeleteGift(int id)` | Delete, return updated gift list |
| RuffleGift | `Task<string> RuffleGift(GiftModel gift)` | Set IsRaffled = true on gift |
| CheckGiftPayed | `Task<IEnumerable<CartItemModel>> CheckGiftPayed(int id)` | Get all paid CartItems for this gift |
| SortByBuyers | `Task<IEnumerable<GiftDto.GiftBuyersData>> SortByBuyers()` | Projection: gift + buyer count |

**Eager load**: Gift → `.Include(g => g.Donor)`, `.Include(g => g.Category)`

---

## ILotteryRepository / LotteryRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetRaffleEntries | `Task<IEnumerable<RaffleDto.RaffleEntry>> GetRaffleEntries(int giftId)` | All paid purchases for a gift (each ticket = 1 entry) |
| SetWinner | `Task<string> SetWinner(int giftId, int winnerUserId)` | Record the winner, return Hebrew confirmation |
| GetWinnerDetails | `Task<UserModel> GetWinnerDetails(int userId)` | Load winner's user record |

**Logic note**: `GetRaffleEntries` returns one `RaffleEntry` per ticket quantity unit (not per purchase row) so the random draw is weighted by quantity.

---

## ISalesRepository / SalesRepository

| Method | Signature | Description |
|--------|-----------|-------------|
| GetGiftResumes | `Task<List<SalesDto.GiftResume>> GetGiftResumes()` | Per-gift: total tickets sold, total revenue |
| GetGiftWithBuyers | `Task<List<SalesDto.GiftsWithBuyers>> GetGiftWithBuyers()` | Per-gift: list of buyers and their quantities |
| GetGiftsByPriceDesc | `Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByPriceDesc()` | Same as above, sorted by price descending |
| GetGiftsByQuantityDesc | `Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByQuantityDesc()` | Same as above, sorted by total sold descending |

**Note**: This is a **read-only** repository — no write operations. All data comes from joining Gifts, CartItems, and Carts (paid only).

**DTO shapes**:
```csharp
// GiftResume — summary row per gift
class GiftResume { int GiftId; string GiftName; int TotalSold; decimal TotalRevenue; }

// GiftsWithBuyers — gift + list of buyers
class GiftsWithBuyers { GiftDto Gift; List<BuyerInfo> Buyers; }
class BuyerInfo { int UserId; string FullName; int Quantity; }
```
