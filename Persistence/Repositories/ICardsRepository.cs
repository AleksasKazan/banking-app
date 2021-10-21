using System;
using System.Threading.Tasks;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public interface ICardsRepository
    {
        Task<int> CreateCard(CardWriteModel card);
    }
}
