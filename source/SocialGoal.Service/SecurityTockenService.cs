using System;
using System.Collections.Generic;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;

namespace SocialGoal.Service
{
    public interface ISecurityTokenService
    {
        IEnumerable<SecurityToken> GetSecurityTokens();
        Guid GetSecurityToken(Guid id);
        int GetActualId(Guid id);
        void CreateSecurityToken(SecurityToken securityToken);
        void DeleteSecurityToken(Guid id);
        void SaveSecurityToken();
    }

    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly ISecurityTokenRepository securityTokenRepository;
        private readonly IUnitOfWork unitOfWork;

        public SecurityTokenService(ISecurityTokenRepository securityTokenRepository, IUnitOfWork unitOfWork)
        {
            this.securityTokenRepository = securityTokenRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ISecurityTokenService Members

        public IEnumerable<SecurityToken> GetSecurityTokens()
        {
            var securityToken = securityTokenRepository.GetAll();
            return securityToken;
        }

        public Guid GetSecurityToken(Guid id)
        {
            var securityToken = securityTokenRepository.Get(s => s.Token == id).Token;
            if (securityToken != null)
            {
                return securityToken;
            }
            else
            {
                Guid newguid = Guid.NewGuid();
                return newguid;
            }
        }
        public int GetActualId(Guid id)
        {
            var actualId = securityTokenRepository.Get(s => s.Token == id).ActualID;
            return actualId;
        }
        public void CreateSecurityToken(SecurityToken securityToken)
        {
            securityTokenRepository.Add(securityToken);
            SaveSecurityToken();
        }

        public void DeleteSecurityToken(Guid id)
        {
            var securityToken = securityTokenRepository.Get(s => s.Token == id);
            securityTokenRepository.Delete(securityToken);
            SaveSecurityToken();
        }

        public void SaveSecurityToken()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
