﻿using Not.Domain.Base;
using Not.Domain.Exceptions;

namespace NTS.Domain.LocalizationDemo;

public class LocalizationTest : DomainEntity
{
    public LocalizationTest()
        : base(GenerateId()) { }

    public string Success()
    {
        return "My dick Yanko";
    }

    public string Invalid()
    {
        throw new DomainException("Kur {0}", "debel");
    }
}
