﻿using EnduranceJudge.Core.Models;
using EnduranceJudge.Domain.Core.Exceptions;
using System;

namespace EnduranceJudge.Domain.Core.Models
{
    public abstract class DomainObjectBase<TException> : IDomainObject, IEquatable<DomainObjectBase<TException>>
        where TException : DomainObjectException, new()
    {
        protected const string GENERATE_ID = "GenerateIdFlag";

        protected DomainObjectBase() {}
        protected DomainObjectBase(string flag)
        {
            this.Id = DomainIdProvider.Generate();
        }

        public int Id { get; private init; }

        internal void Validate(Action action)
        {
            try
            {
                action();
            }
            catch (DomainException exception)
            {
                this.Throw(exception.Message);
            }
        }

        internal void Throw(string message)
            => Thrower.Throw<TException>(message);

        public override bool Equals(object other)
            => this.IsEqual(other);
        //
        // public bool Equals(IIdentifiable identifiable)
        //     => this.IsEqual(identifiable);

        public bool Equals(DomainObjectBase<TException> domainObject)
            => this.IsEqual(domainObject);

        public static bool operator ==(DomainObjectBase<TException> one, DomainObjectBase<TException> two)
        {
            if (ReferenceEquals(one, null))
            {
                return ReferenceEquals(two, null);
            }
            return one.Equals(two);
        }

        public static bool operator !=(DomainObjectBase<TException> one, DomainObjectBase<TException> two)
        {
            if (ReferenceEquals(one, null))
            {
                return ReferenceEquals(two, null);
            }
            return !(one == two);
        }

        private bool IsEqual(object other)
        {
            if (other is not IDomainObject domainModel)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (this.Id == domainModel.Id)
            {
                return true;
            }
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
            => base.GetHashCode() + this.Id;
    }
}
