using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Payment.Model;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MemApp.Application.PaymentGateway.SslCommerz.Command
{
    public class CreateOrderValidationCommand:IRequest<TransactionVm>
    {
        public OrderValidationReq Model { get; set; } = new OrderValidationReq();
    }
    public class CreateOrderValidationCommandHandler : IRequestHandler<CreateOrderValidationCommand, TransactionVm>
    {
        private readonly IMemDbContext _memdbcontext;
        protected List<string> key_list;
        protected string error;
        protected string generated_hash;
        private string SSLCz_URL = "https://sandbox.sslcommerz.com/";
        public CreateOrderValidationCommandHandler(IMemDbContext memDbContext)
        {
            _memdbcontext = memDbContext;
        }
        public bool ipn_hash_verify(HttpRequest req)
        {
            if (req.Form["verify_sign"] != "" && req.Form["verify_key"] != "")
            {
                string verify_key = req.Form["verify_key"];
                if (verify_key != "")
                {
                    key_list = verify_key.Split(',').ToList<string>();
                    List<KeyValuePair<string, string>> data_array = new List<KeyValuePair<string, string>>();
                    foreach (string k in key_list)
                    {
                        data_array.Add(new KeyValuePair<string, string>(k, req.Form[k]));
                    }
                    string hashed_pass = MD5("media6203aa114fa1f@ssl");
                    data_array.Add(new KeyValuePair<string, string>("store_passwd", hashed_pass));
                    data_array.Sort(
                        delegate (KeyValuePair<string, string> pair1,
                        KeyValuePair<string, string> pair2)
                        {
                            return pair1.Key.CompareTo(pair2.Key);
                        }
                    );
                    string hash_string = "";
                    foreach (var kv in data_array)
                    {
                        hash_string += kv.Key + "=" + kv.Value + "&";
                    }
                    hash_string = hash_string.TrimEnd('&');
                    generated_hash = this.MD5(hash_string);
                    if (generated_hash == req.Form["verify_sign"])
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public string MD5(string s)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(s);
            byte[] hashedBytes = System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedString;
        }
        public bool OrderValidate(string MerchantTrxID, string MerchantTrxAmount, string MerchantTrxCurrency, HttpRequest req)
        {
            bool hash_verified = this.ipn_hash_verify(req);
            if (hash_verified)
            {
                string json = string.Empty;
                string EncodedValID = System.Web.HttpUtility.UrlEncode(req.Form["val_id"]);
                string EncodedStoreID = System.Web.HttpUtility.UrlEncode("media6203aa114fa1f");
                string EncodedStorePassword = System.Web.HttpUtility.UrlEncode("media6203aa114fa1f@ssl");

                string validate_url = SSLCz_URL + "validator/api/validationserverAPI.php" + "?val_id=" + EncodedValID + "&store_id=" + EncodedStoreID + "&store_passwd=" + EncodedStorePassword + "&v=1&format=json";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(validate_url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(resStream))
                {
                    json = reader.ReadToEnd();
                }
                if (json != "")
                {
                    SSLCommerzValidatorResponse resp = JsonConvert.DeserializeObject<SSLCommerzValidatorResponse>(json);

                    if (resp?.status == "VALID" || resp?.status == "VALIDATED")
                    {
                        if (MerchantTrxCurrency == "BDT")
                        {
                            if (MerchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(MerchantTrxAmount) - Convert.ToDecimal(resp.amount)) < 1) && MerchantTrxCurrency == "BDT")
                            {
                                return true;
                            }
                            else
                            {
                                error = "Amount not matching";
                                return false;
                            }
                        }
                        else
                        {
                            if (MerchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(MerchantTrxAmount) - Convert.ToDecimal(resp.currency_amount)) < 1) && MerchantTrxCurrency == resp.currency_type)
                            {
                                return true;
                            }
                            else
                            {
                                this.error = "Currency Amount not matching";
                                return false;
                            }

                        }
                    }
                    else
                    {
                        this.error = "This transaction is either expired or fails";
                        return false;
                    }
                }
                else
                {
                    this.error = "Unable to get Transaction JSON status";
                    return false;
                }
            }
            else
            {
                this.error = "Unable to verify hash";
                return false;
            }
        }
        public async Task<TransactionVm>  Handle(CreateOrderValidationCommand request, CancellationToken cancellationToken)
        {
            var result = new TransactionVm();
            bool response = OrderValidate(request.Model.tran_id, request.Model.ammount, request.Model.currency , request.Model.request);
            if (response)
            {
                Console.WriteLine("ohNo its happend");

               //create transaction in db
            }
            return result;
        }
    }
}
