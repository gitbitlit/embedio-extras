﻿namespace Unosquare.Labs.EmbedIO.BearerToken
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    /// <summary>
    /// Context to share data with AuthorizationServerProvider.
    /// </summary>
    public class ValidateClientAuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateClientAuthenticationContext"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <exception cref="System.ArgumentNullException">httpContext.</exception>
        public ValidateClientAuthenticationContext(IHttpContext httpContext)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

            Identity = new ClaimsIdentity();
        }

        /// <summary>
        /// The Client Id.
        /// </summary>
        public string IdentityName { get; protected set; }

        /// <summary>
        /// Flags if the Validation has errors.
        /// </summary>
        public bool HasError { get; protected set; }

        /// <summary>
        /// Indicates if the Validation is right.
        /// </summary>
        public bool IsValidated { get; protected set; }

        /// <summary>
        /// Http Context instance.
        /// </summary>
        public IHttpContext HttpContext { get; protected set; }

        /// <summary>
        /// Claims.
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Rejects a validation.
        /// </summary>
        public void Rejected()
        {
            IsValidated = false;
            HasError = true;
        }

        /// <summary>
        /// Validates credentials with identity name.
        /// </summary>
        /// <param name="identityName">Name of the identity.</param>
        public void Validated(string identityName = null)
        {
            IdentityName = identityName;
            Identity.AddClaim(new Claim(ClaimTypes.Name, identityName));
            IsValidated = true;
            HasError = false;
        }

        /// <summary>
        /// Retrieve JsonWebToken.
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="expires">The expires.</param>
        /// <returns>
        /// The token string.
        /// </returns>
        public string GetToken(SymmetricSecurityKey secretKey, DateTime? expires = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var plainToken = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = Identity,
                Issuer = "Embedio Bearer Token",
                Expires = expires,
                SigningCredentials = new SigningCredentials(secretKey,
                    SecurityAlgorithms.HmacSha256Signature),
            });

            return tokenHandler.WriteToken(plainToken);
        }
    }
}