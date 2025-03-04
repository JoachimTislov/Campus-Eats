# Relations between modules and contexts

## Flow Chart

```mermaid
flowchart TD

    AdminContext -.CourierRoleRequestEvaluated.-> UserContext
    AdminContext -.ProductCRUDOperation.-> ProductContext
    AdminContext -.AdminInvitationEvent.-> NotificationContext
    AdminContext -.UserAcceptedAdminInvitation.-> NotificationContext
    AdminContext -.CourierRoleRequestEvaluated.-> NotificationContext

    UserContext -.UserResponseToAdminInvitation.-> AdminContext
    UserContext -.UserAppliedForCourierRole.-> AdminContext

    subgraph Campus-Eats
        UserContext -.UserRegistered.-> NotificationContext
        NotificationContext --Notifications--> UserContext

        ProductContext -.FoodItemPriceChanged.-> CartContext
        ProductContext -.FoodItemNameChanged.-> CartContext

        ProductContext -.Product-Edit/Created.-> StripeContext 
        OrderContext -.CheckoutSession.-> StripeContext
        
        CartContext --CartCheckedOut--> IOrderingService

        subgraph OrderContext
            IOrderingService --> Order
        end 

        Order -.OrderPlaced.-> FulfillmentContext
        Order -.OrderStatusChanged.-> FulfillmentContext

        Order -.OrderPlaced.-> InvoicingContext
        Order -.OrderStatusChanged.-> InvoicingContext

        Order -.OrderStatusChanged.-> NotificationContext
    end    
```

## Contexts

### Admin Context

Admin is created by the system and thereby a default account with name: Admin, and a *secure password* - "Test1234*". The User account has following admin role in ASP.NET - Identity.

The Administrator can invite new users to become an admin, preform CRUD operation on the products, accept or deny courier requests and change delivery fee

Handlers:

* UserAppliedForCourierRoleHandler - Creates courier request
* UserResponseToAdminInvitationHandler - Creates an admin account if users accepts the invitation

Events:

* AdminInvitationEvent - An invite to a trusted user, sent by notification context
* UserAcceptedAdminInvitation - Handled by the notification context
* CourierRoleRequestEvaluated - Handled By user and notification context

#### RetrieveDashBoardData

* Collect all orders from OrderContext
* Gather information about earnings from InvoicingContext

```mermaid
classDiagram
    direction LR

    class IAdminInvitationRepository {
        <<Repository>>
        + SendAdminInvitation()
        + GetAdminInvitationByIdAsync()
    }
    IAdminInvitationRepository --> IAdminService

    class ICourierRequestRepository {
        <<Repository>>
        + CreateCourierRoleRequest()
        + GetCourierRequestById()
        + GetCourierRoleRequestByUserId()
        + GetPendingCourierRoleRequestsAsync()
    }
    ICourierRequestRepository --> IAdminService

    class ConfigManager {
        <<Manager>>
        + GetConfig()
        + UpdateConfig()
    }
    ConfigManager --> Config
    ConfigManager --> IAdminService

    class Config {
        <<Aggregate root>>
        + DeliveryFee
    }

    class IAdminService {
        <<Service>>
        + CreateAdminAccount()
        + GetAdminByName()
        + Login()
        + GeneratePasswordResetTokenAsync()
        + CreateCourierRoleRequest()
        + GetCourierRoleRequestByUserId()
        + GetCourierRolePendingRequestsAsync()
        + RespondToCourierRequest()
        + SendAdminInvitation()
        + GetAdminInvitationByIdAsync()
        + RespondToInvitationAsync()
        + decimal GetDeliveryFee()
        + void SetDeliveryFee()
    }

    class Admin {
        + Explicit Constructor*
    }

    class AdminInvitation {
        <<Aggregate root>>
        + Id
        + UserId
        + Resume
        + Status 
        + Declined()
        + Approved()
    }

    class AdminInvitationStatusEnum {
        <<enum>>
        Pending
        Declined
        Approved
    }

    class CourierRoleRequest {
        <<Aggregate root>>
        + Id
        + UserId
        + Resume 
        + Status
        + Declined()
        + Approved()
        + IsApproved()
    }

    class CourierRoleRequestStatusEnum {
        <<enum>>
        Pending
        Declined
        Approved
    }

    class RequestActionEnum {
        <<enum>>
        Accept
        Reject
    }

    class UserInUserContext {
        <<Aggregate Root>>
        + FirstName
        + LastName
        + Address
        + LastLoginDate
        + HasChangedPassword
    }

    AdminInvitation --> AdminInvitationStatusEnum
    CourierRoleRequest --> CourierRoleRequestStatusEnum

    IAdminService --> Admin
    IAdminService --> AdminInvitation
    IAdminService --> CourierRoleRequest
    Admin <-- UserInUserContext
```

### User Context

A User can be either a costumer or a courier with following role in ASP.NET - Identity. "Customers and couriers should be able to register as a user on our site." - Technical requirement

Github IdentityUser - <https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.EntityFramework/IdentityUser.cs>.

Handlers:

* CourierRoleRequestEvaluatedHandler - Assigns courier role if accepted

Events:

* UserRegistered - Handled by notification context, sends mail to the user
* UserAppliedForCourierRole - Add the request to a list (Handled by a Administrator)
* UserResponseToAdminInvitation - Handled by admin context

```mermaid
classDiagram
    direction LR

    class IUserRepository {
        <<Repository>>
        + CreateUser()
        + UpdateUser()
        + UpdateUserProfile()
        + UpdateUserAddress()
        + GetUsersByRole()
        + GetUserByClaimsPrincipal()
        + GetUserByEmail()
        + GetUserByName()
        + GetUserByHttpContext()
        + GetEmailByClaims()
        + GetClaimTypeValue()
        + GetExternalLoginValuesAsync()
        + GetUserById()
        + GetCustomers()
    }

    class IUserService {
        <<Service>>
        + CheckIfUserIsAssignedRole()
        + AssignRoleToUserAsync()
        + Register()
        + Login()
        + CheckPassword()
        + GeneratePasswordResetTokenAsync()
        + ChangePassword()
        + LoginWithThirdParty()
    }

    class User {
        <<Aggregate Root>>
        + FirstName
        + LastName
        + Address
        + LastLoginDate
        + HasChangedPassword
    }

    class Address {
        <<Owned>>
        + AddressLine
        + City
        + PostalCode
    }

    class IdentityUser {
        + string Id
        + string UserName
        + string NormalizedUserName
        + string Email
        + string NormalizedEmail
        + bool EmailConfirmed
        + string PasswordHash
        + string SecurityStamp
        + string ConcurrencyStamp
        + string PhoneNumber
        + bool PhoneNumberConfirmed
        + bool TwoFactorEnabled
        + DateTimeOffset? LockoutEnd
        + bool LockoutEnabled
        + int AccessFailedCount
    }

    IUserRepository --> User
    IUserService --> User
    User --> Address
    User <-- IdentityUser
```

### Product Context

Events:

* FoodItemPriceChanged and FoodItemNameChanged - CartContext updates their items accordingly
* ProductCreated and ProductEdited - Handled by stripe context

```mermaid
classDiagram
    direction LR

    class IProductRepository {
        <<Repository>>
        + CreateFoodItem()
        + EditFoodItem()
        + DeleteFoodItem()
        + GetFoodItemById()
        + GetAllFoodItems()
        + UpdateStripeProductId()
    }

    class IProductService {
        <<Service>>
        + ValidateImageFile()
        + CreateFoodItemAsync()
        + GetFoodItemByIdAsync()
        + EditFoodItemAsync()
        + DeleteFoodItemAsync()
    }

    class FoodItem {
        <<Aggregate Root>>
        + Id
        + Name
        + Description
        + ImageLink
        + Price
        + Stripe_productId
        + EditFoodItem()
    }
    IProductRepository --> IProductService
    IProductService --> FoodItem
```

### Cart Context

Events:

* CustomerCheckedOutTheirCart - A custom confirms an order, handled by OrderContext

```mermaid
classDiagram
    direction LR

    class ICartRepository {
        <<Repository>>
        + GetCartAsync()
        + GetCartsWithSpecificItemAsync()
        + UpdateCartAsync()
        + UpdateCartsAsync()
        + AddItemToCartAsync()
        + IncrementCartItemCountAsync()
        + DecrementCartItemCountOrDeleteIfZeroAsync()
        + DeleteCartItemFromCartAsync()
        + DeleteCartAsync()
    }

    class ICartService {
        <<Service>>
        + GetCartIdFromSessionAsync()
        + GetCartAsync()
        + GetCartsWithSpecificItemAsync()
        + AddItemToCartAsync()
        + IncrementCartItemCountAsync()
        + DecrementCartItemCountOrDeleteIfZeroAsync()
        + DeleteCartItemFromCartAsync()
        + UpdateCartsAsync()
        + GetCartSubtotal()
        + DeleteCartAsync()
        + CheckoutCart()
    }
    
    class Cart {
        <<Aggregate root>>
        + Id
        + Items
        + AddItem()
        + IncrementCountOfItem()
        + RemoveItem()
        + DeleteItemFromCart()
        + GetSubtotal()
    }

    class CartItem {
        + Id
        + CartId
        + Sku
        + Name
        + Price
        + Sum --> Price * Count
        + Count
        + ImageLink
        + Stripe_productId
        + AddOne()
        + RemoveOne()
        + IsCountOne()
    }

    ICartRepository --> ICartService
    ICartService --> Cart
    Cart --> CartItem
```

### Ordering Context

Handlers:

* CartCheckedOutHandler - Creates an order with corresponding items from cart

Events:

* OrderPlaced - The Order has been finalized, converted from CartContext data to an Order.
* OrderStatusChanged - Fulfillment, invoicing and notification context handles this event

```mermaid
classDiagram
    direction LR

    class IOrderingRepository {
        <<Repository>>
        + CreateOrder()
        + GetOrderById()
        + UpdateOrder()
        + SetOrderStatus()
        + SetCourier()
        + SetCampusLocation()
        + SetTip()
        + GetOrdersByCustomerId()
        + GetOrdersByOrderStatus()
        + GetOrdersByCourierId()
        + GetAllOrders()
    }

    class IOrderingService {
        <<Service>>
        + CreateOrder()
        + GetOrders()
        + GetOrderById()
        + PlaceOrder()
        + PickOrder()
        + SetTipForOrder()
        + GetOrderHistoryByUserId()
        + CanCancelOrder()
        + GetQuantityOfOrdersInEachStage()
        + SetOrderStatus()
        + GetDeliveryFeeForGivenOrderByOrderId()
        + GetMonthOrders()
    }

    class Order {
        <<Aggregate root>>
        + Id
        + OrderDate
        + CampusLocation
        + CustomerId
        + CourierId
        + Tip
        + DeliveryFee
        + CourierDeliveryFeeCut
        + AdminDeliveryFeeCut
        + CourierEarning
        + Status
        + OrderLines
        + SetDeliveryFee()
        + SetTip()
        + SetStatus()
        + IsTipped()
        + TotalCost()
        + SubtotalCost()
        + CanCancelOrder()
        + TotalAdminBenefit()
        + AddOrderLine()
        + IsNew()
        + IsCanceled()
        + IsDelivered()
        + SetCourier()
    }
   
    class OrderLine {
        + Id
        + OrderId
        + Name
        + Price
        + Quantity
        + Stripe_productId
    }

    class CampusLocation {
        <<ValueObject>>
        + Building
        + RoomNumber
        + Notes
    }

    class OrderStatus {
        <<enum>>
        New
        Placed
        Picked
        Shipped
        Delivered
        Missing
        Canceled
    }

    IOrderingRepository --> IOrderingService
    IOrderingService --> Order
    Order --> CampusLocation
    Order --> OrderLine
    Order --> OrderStatus
```

### Fulfillment Context

Handlers:

* InvoicePaidHandler - Makes an offer refundable
* OrderStatusChangedHandler - Creates, refunds and cancels offers depending on the order status
  
```mermaid
classDiagram
    direction LR

    class IFulfillmentRepository {
        <<Repository>>
        + CreateOffer()
        + GetOfferByOrderId()
        + UpdateOffer()
    }

     class IFulfillmentService {
        <<Service>>
        + CreateOffer()
        + UpdateOffer()
        + GetOfferByOrderId()
        + UpdateOfferToRefundable()
        + CancelOfferSinceOrderHasBeenShipped()
        + RefundOrderExpenseToCostumer()
    }

    class Offer {
        <<Aggregate root>>
        + Id
        + OrderId
        + CustomerId
        + OrderExpense
        + Status
        + IsRefundable()
        + IsRefunded()
        + IsCanceled()
        + Refundable()
        + CancelOffer()
        + RefundOrderExpenseToCustomer()
    }

    class OfferStatus {
        <<enum>>
        Unpaid
        Paid
        Canceled
        Refundable
    }

    IFulfillmentRepository --> IFulfillmentService
    IFulfillmentService --> Offer
    Offer --> OfferStatus
```

### Invoicing Context

Handlers:

* OrderPlacedHandler - Creates an invoice
* OrderStatusChangedHandler - Credits or removes order expense from payment and keeps the delivery in special case of order canceled and old status was picked.

Events:

* InvoicePaid - Handled by the fulfillment context
  
```mermaid
classDiagram
    direction LR

    class IInvoicingRepository {
        <<Repository>>
        + CreateInvoice()
        + UpdateInvoiceStatusById()
        + HandleTransactionResult()
        + GetInvoiceById()
        + GetInvoiceByOrderId()
        + GetSortedInvoices()
        + CreditInvoiceByOrderId()
        + CancelOrderPaymentOfInvoiceAndReplaceWithDeliveryFee()
    }

    class Invoice {
        <<Aggregate root>>
        + Id
        + PaymentDue
        + Customer
        + OrderId
        + Address
        + Status
        + SetPaymentDue()
        + SetAddress()
        + SetStatus()
        + IsPayable()
        + IsPaid()
        + CreditInvoice()
    }

    class InvoiceAddress {
        <<ValueObject>>
        + AddressLine
        + City
        + PostalCode
    }

    class InvoiceStatusEnum {
        <<Enum>>
        Pending
        Paid
        TransactionFailed
        Credited
    }

    IInvoicingRepository --> Invoice
    Invoice --> InvoiceAddress
    Invoice --> InvoiceStatusEnum
```

### Notification Context

#### Each email notifications mentioned in the project description

Handlers:

* AdminInvitationHandler - Sends mail, creates notifications, and notifies the user about the invite
* UserRegisteredHandler - Sends mail if Send mail is true and creates a notification
* CourierRequestEvaluatedHandler - Notifies user of admin response
* OrderStatusChangedHandler - Sends messages related to the updated order status
* UserAcceptedAdminInvitationHandler - Notifies the user about the new Admin account

#### Push-notifications

Only push-notification will use the signalR tools, client function: NotifyClient() - Will alert the customer and courier related to an order in real time

```mermaid
classDiagram 
    direction LR

    class IEmailService {
        <<Service>>
        + SendEmailAsync()
    }

    class INotificationClient {
        <<ClientService>>
        + NotifyClient() 
        + ReloadPage()
    }

    class NotificationHub {
        <<Aggregate root>>
        + OnConnected()
        + OnDisconnected()
    }

    class INotificationRepository {
        <<Repository>>
        + CreateAlertNotification()
        + GetAlertNotificationsByUserId()
        + CreateNotification()
        + GetNotificationById()
        + GetNotificationsFilteredByUserId()
    }

    class INotificationService {
        <<Service>>
        + SendQueuedNotifications()
        + CreateAlertNotification()
        + CreateNotification()
        + GetNotificationById()
        + GetNotificationsFilteredByUserId()
        + NotifyUser()
        + ReloadClientPage()
    }

    class Notification {
        <<Aggregate root>>
        + Id
        + UserId
        + Title
        + Description
        + Link
    }

    class AlertNotification {
        <<Aggregate root>>
        + Id
        + UserId
        + Message
        + Link
        + NameOfLink
    }

    INotificationRepository --> INotificationService
    NotificationHub --> INotificationService
    INotificationService --> Notification
    INotificationService --> AlertNotification
    INotificationClient --> NotificationHub
```

### Stripe Context

Handlers:

* ProductCreatedHandler - Create a corresponding stripe product with the same information
* ProductEditedHandler - Updates the stripe product accordingly

```mermaid
classDiagram 
    direction LR

    class IStripeSessionService {
        <<Service>>
        + PayInvoiceThroughACheckoutSession()
        + PayTipThroughACheckoutSession()
    }

    class IStripeProductService {
        <<Service>>
        + CreateProduct()
        + EditProductById()
        + GetProductById(string productId)
    }
```

### Generic DB Context

Most of interactions with the database will go through the generic repository.
A repository for each context prevents reuse of code which is optimal. It's about creating as many references to one operation rather than writing it different places in the code, since when you later want to change the logic of the operation its only one area to change.

This repository also allows us to view all the specific interactions with the database from the contexts.

```mermaid
classDiagram
    class IDbRepository {
        <<Repository>>
        + AddAsync()
        + Remove()
        + RemoveRange()
        + Update()
        + UpdateRange()
        + GetByIdAsync()
        + All()
        + AnyAsync()
        + WhereToListAsync()
        + IncludeToListAsync()
        + WhereFirstOrDefaultAsync()
        + IncludeWhereFirstOrDefaultAsync()
        + IncludeOrderByToListAsync()
        + IncludeWhereToListAsync()
        + WhereOrderByToListAsync()
        + WhereOrderByDescendingToListAsync()
        + IncludeWhereOrderByToListAsync()
    }

    class AnyRepositoryInThisCodeBase {
        <<Repository>>
        *Methods*
    }

    IDbRepository --> AnyRepositoryInThisCodeBase
```
