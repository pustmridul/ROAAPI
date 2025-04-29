using Azure.Core;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.SubscriptionPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Commands.RoaSubscriptionVerify
{
    public class SubscriptionDetailsVerify
    {
        private readonly IMemDbContext _context;

        public SubscriptionDetailsVerify(IMemDbContext context)
        {
            _context = context;
        }

        public bool MonthVerify(int MemberId, List<MemberPaymentReqSSl> data)
        {
            var checkedList = data!.Where(q => q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();
            var uncheckedList = data!.Where(q => !q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();

            foreach (var item in checkedList)
            {
                if (item.SubscriptionMonth > uncheckedList.FirstOrDefault()?.SubscriptionMonth)
                {
                   
                    return false;
                }
            }
            // need to fix

            if (checkedList.Count == 0)
            {
              
                return false;
            }
            foreach (var s in data.Where(q => q.IsChecked))
            {
                var checkExist = _context.ROASubscriptionPaymentDetail
                      .AsNoTracking()
                      .Any(x => x.MemberId == MemberId && x.SubscriptionMonth == s.SubscriptionMonth && x.SubscriptionYear == s.SubscriptionYear);

                if (checkExist)
                {
                    return false;
                }
            }
                return true;
        }

        public bool TotalAmountCheck()
        {
            return true;
        }
    }
}
