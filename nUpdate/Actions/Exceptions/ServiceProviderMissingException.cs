// ServiceProviderMissingException.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.Actions.Exceptions
{
    public class ServiceProviderMissingException : Exception
    {
        public ServiceProviderMissingException() : base("Missing service provider in the executing assembly.")
        {
        }

        public ServiceProviderMissingException(string serviceType) : base(
            $"Missing service provider in the executing assembly: {serviceType}")
        {
        }
    }
}