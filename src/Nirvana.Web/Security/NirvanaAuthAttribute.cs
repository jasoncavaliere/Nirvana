﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Nirvana.Security;

namespace Nirvana.Web.Security
{
//    public class NirvanaAuthAttribute : AuthorizeAttribute
//    {
//        public NirvanaAuthAttribute( AccessType requiredAccessType, Type claimType = null)
//        {
//            RequiredAccessType = requiredAccessType;
//            if (claimType != null)
//            {
//                RequiredClaimType = Activator.CreateInstance(claimType) as ClaimType;
//            }
//        }
//
//        public IClaimAuthorizer Authorizer { get; set; }
//        public AccessType RequiredAccessType { get; set; }
//        public ClaimType RequiredClaimType { get; set; }
//        public override void OnAuthorization(HttpActionContext actionContext)
//        {
//            //Validates Token
//            base.OnAuthorization(actionContext);
//
//            if (RequiredAccessType == AccessType.Any || RequiredClaimType == null)
//            {
//                return;
//            }
//            var user = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
//            var userId = user?.Claims?.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
//            if (Authorizer.CheckByUserId(userId, RequiredClaimType, RequiredAccessType))
//            {
//                return;
//            }
//            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
//        }
//    }
}
