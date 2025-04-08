using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace MemApp.Application.PaymentGateway.SslCommerz.Command
{

    public class CreateInitiatePaymentCommand : IRequest<SSLCommerzInitResponse>
    {
        public SSLCommerzInitRequest Model { get; set; } = new SSLCommerzInitRequest();
    }
    public class CreateInitiatePaymentCommandHandler : IRequestHandler<CreateInitiatePaymentCommand, SSLCommerzInitResponse>
    {
        private readonly IMemDbContext _context;
        protected string generated_hash;
        protected List<string> key_list;
        public CreateInitiatePaymentCommandHandler(IMemDbContext context)
        {
            _context = context;
        }
        private static Dictionary<string, string> GetProperties(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
                return props;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(obj, new object[] { });

                if (val != null)
                {
                    props.Add(prop.Name, val.ToString());
                }
            }

            return props;
        }

        public SSLCommerzInitResponse InitiateTransaction(NameValueCollection PostData, bool GetGateWayList = false)
        {
            PostData.Add("store_id", "flexa656d4f9a5f99e");
            PostData.Add("store_passwd", "flexa656d4f9a5f99e@ssl");
            PostData.Add("tran_id", GenerateUniqueId());
            PostData.Add("product_category", "TopUp");
            PostData.Add("product_name", "TopUp");
            PostData.Add("product_profile", "TopUp");
            PostData.Add("success_url", "http://localhost:7118/api/SslCommerz/PaymentOrderValidation/order-validation");
            PostData.Add("fail_url", "http://localhost:7118/api/SslCommerz/PaymentOrderValidation/order-validation");
            PostData.Add("cancel_url", "http://localhost:7118/api/SslCommerz/PaymentOrderValidation/order-validation");
            PostData.Add("shipping_method", "SSL");
            PostData.Add("ship_name", "test");
            PostData.Add("ship_add1", "test");
            PostData.Add("ship_city", "test");
            PostData.Add("ship_state", "test");
            PostData.Add("ship_postcode", "test");
            PostData.Add("ship_country", "test");
            string response = SendPost(PostData);
            try
            {
                SSLCommerzInitResponse resp = JsonConvert.DeserializeObject<SSLCommerzInitResponse>(response);
                if (resp.status == "SUCCESS")
                {
                    return resp;
                }
                else
                {
                    throw new Exception("Unable to get data from SSLCommerz. Please contact your manager!");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }

        private string GenerateUniqueId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= (b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks).ToUpper();
        }
        protected string SendPost(NameValueCollection PostData)
        {
            string response = Post("https://sandbox.sslcommerz.com/gwprocess/v3/api.php", PostData);
            return response;
        }
        public static string Post(string uri, NameValueCollection PostData)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, PostData);
            }
            return System.Text.Encoding.UTF8.GetString(response);
        }

        public async Task<SSLCommerzInitResponse> Handle(CreateInitiatePaymentCommand request, CancellationToken cancellation)
        {
            NameValueCollection PostData = new NameValueCollection();

            foreach (var item in GetProperties(request.Model))
            {
                PostData.Add(item.Key, item.Value);
            }
            var response = InitiateTransaction(PostData);

            if (string.IsNullOrEmpty(response.GatewayPageURL))
            {
                return (new SSLCommerzInitResponse
                {
                    status = "fail",
                    storeLogo = response.storeLogo
                });
            }
            return (new SSLCommerzInitResponse
            {
                status = "success",
                sessionkey = response.sessionkey,
                GatewayPageURL = response.GatewayPageURL,
                storeLogo = response.storeLogo
            });
        }

    }
}
