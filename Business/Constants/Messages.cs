using Core.Entities.Concrete;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.MarketDtos;
using Entities.Dtos.SaleOrderDtos;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        public static string UserNotFound = "Kullanıcı bulunamadı";
        public static string PasswordError = "Şifre hatalı";
        public static string SuccessDefaultLogin = "Sisteme giriş başarılı";
        public static string UserAlreadyExists = "Böyle bir email sistemde mevcut";
        public static string UserRegistered = "Kullanıcı başarılı bir şekilde kaydedildi.";
        public static string AccessTokenCreated = "Access Token başarılı şekilde oluşturuldu";
        public static string UserAdded = "Kullanıcı başarılı bir şekilde eklendi";
        public static string UserRole = "Kullanıcı rolü eklendi";
        public static string UserStatus = "Kullanıcı status güncellendi";
        public static string UserControl = "Bu kullanıcı adı sistemde mevcut";
        public static string ChangePassword = "Şifreniz değiştirildi";
        public static string WalletAdded = "Cüzdan eklendi";
        public static string Parity = "Parite eklendi";
        public static string Price = "Girilen fiyat kabul edilmedi";
        public static string InsufficientBalance = "Yetersiz bakiye";
        public static string BuyOrder = "Alış emri oluşturuldu.";
        public static string ParityList = "Parite listeleme başarılı";
        public static string SaleOrder = "Satış emri oluşturuldu.";
        public static string SaleOrderList = "Listeleme başarılı";
        public static string Coin = "Böyle bir coin yok";
        public static string BuyError = "Eşleşen bir emir yok";
        public static string TotalPriceError = "Toplam fiyat eşleşmedi";
        public static string WrongPassword = "Şifreler aynı değil";
        public static string UserUpdate = "Güncelleme başarılı";
        public static string List = "Listeleme başarılı";
        public static string ErrorList = "Listeleme başarısız";
        public static string BuyOrderCancel = "Emir iptal edildi";
        public static string OrderNotFound = "Emir iptal edilmedi";
        public static string SaleOrderCancel = "Satış emiri iptal edildi";
        public static string ParityListFailed = "Parite listeleme başarısız";
        public static string CoinAdded = "Coin eklendi";
        public static string UserOrdersNotFound = "Bu kullanıcıya ait bir emir yok";
        public static string PhoneAlready = "Bu telefon numarası sistemde mevcut";
        public static string WalletError = "Böyle bir cüzdan yok";
        public static string ParityError = "Böyle bir parite yok";
        public static string BuyOrderError = "Böyle bir alış emiri yok";
        public static string SaleOrderError = "Böyle bir satış emiri yok";
        public static string Success = "Başarılı";
        public static string UnknownError = "Bilinmeyen hata";
    }
}
