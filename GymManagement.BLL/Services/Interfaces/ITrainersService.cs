using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainersService
    {
        public Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default);
        public Task<Result<TrainerDetailsViewModel>> GetTrainerDetailsAsync(int id, CancellationToken ct = default);
        public Task<Result> CreateTrainerAsync(CreateTrainerViewModel trainer, CancellationToken ct = default);
        public Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int id, CancellationToken ct = default);
        public Task<Result> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
        public Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default);

    }
}
