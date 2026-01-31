using System;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Constants;

public static class CacheProfiles
{
  public const string Defaul10 = "Default10";
  public const string Defaul20 = "Default20";
  public static CacheProfile Profile10 = new CacheProfile
  {
    Duration = 10
  };
  public static CacheProfile Profile20 = new CacheProfile
  {
    Duration = 20
  };
}
