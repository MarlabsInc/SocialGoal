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
        private readonly ISecurityTokenRepository _securityTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SecurityTokenService(ISecurityTokenRepository securityTokenRepository, IUnitOfWork unitOfWork)
        {
            _securityTokenRepository = securityTokenRepository;
            _unitOfWork = unitOfWork;
        }

        #region ISecurityTokenService Members

        public IEnumerable<SecurityToken> GetSecurityTokens()
        {
            var securityToken = _securityTokenRepository.GetAll();
            return securityToken;
        }

        public Guid GetSecurityToken(Guid id)
        {
            var securityToken = _securityTokenRepository.Get(s => s.Token == id).Token;
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
            var actualId = _securityTokenRepository.Get(s => s.Token == id).ActualID;
            return actualId;
        }
        public void CreateSecurityToken(SecurityToken securityToken)
        {
            _securityTokenRepository.Add(securityToken);
            SaveSecurityToken();
        }

        public void DeleteSecurityToken(Guid id)
        {
            var securityToken = _securityTokenRepository.Get(s => s.Token == id);
            _securityTokenRepository.Delete(securityToken);
            SaveSecurityToken();
        }

        public void SaveSecurityToken()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
