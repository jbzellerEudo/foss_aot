// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using Duende.IdentityModel.Client;

namespace Duende.IdentityModel;

public class BasicAuthenticationEncodingTests
{
    [Theory]
    [InlineData("foo", "bar")]
    [InlineData("foo", "very+secret")]
    [InlineData("firstname lastname", "bar")]
    [InlineData("firstname:lastname", "bar")]
    [InlineData("firstname:lastname", "bar:bar2")]
    [InlineData("sören:müller", "bar:bar2")]
    [InlineData(":/&%%(/(&/) %&%&/%/&", ")(/)(/&  /%/&%$&$$&")]
    public void oauth_values_should_decode_correctly(string id, string secret)
    {
        var header = new BasicAuthenticationOAuthHeaderValue(id, secret);
        DecodeOAuthHeader(header.Parameter, out var decodedId, out var decodedSecret);

        decodedId.ShouldBe(id);
        decodedSecret.ShouldBe(secret);
    }

    private void DecodeOAuthHeader(string value, out string id, out string secret)
    {
        var unbased = Unbase64(value);
        var items = unbased.Split(':');

        id = Uri.UnescapeDataString(items[0].Replace("+", "%20"));
        secret = Uri.UnescapeDataString(items[1].Replace("+", "%20"));
    }

    private string Unbase64(string value)
    {
        var unbased = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(unbased);
    }
}
