using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Logics;

namespace FoxTales.Application.Services.Logics;

public class RoundLogic : IRoundLogic
{

    public QuestionDto GetNewCurrentQuestionWithSelectedPlayer(RoomDto room)
    {
        Random rnd = new();
        QuestionDto currentQuestion = room.Questions[rnd.Next(room.Questions.Count)];

        var minSelectionCount = room.Users.Min(u => u.SelectionCount);

        var eligibleUsers = room.Users
            .Where(u => u.SelectionCount == minSelectionCount)
            .ToList();

        PlayerDto selectedPlayer = eligibleUsers[rnd.Next(eligibleUsers.Count)];

        currentQuestion.CurrentUser = selectedPlayer;
        currentQuestion.Text = currentQuestion.Text.Replace("****", selectedPlayer.Username);
        selectedPlayer.SelectionCount += 1;

        return currentQuestion;
    }

    public void UpdateVotePool(PlayerDto voter, PlayerDto owner)
    {
        if (owner.Answer == null)
            throw new InvalidOperationException($"Player {owner.UserId} not provide answer. (UpdateVotePool)");

        owner.Answer.VotersCount++;
        if (!owner.VotersIdsForHisAnswer.Contains(voter.UserId))
            owner.VotersIdsForHisAnswer.Add(voter.UserId);

        var voterIndex = owner.VotesReceived.FindIndex(kv => kv.Key == voter.UserId);

        if (voterIndex >= 0)
            owner.VotesReceived[voterIndex] = new KeyValuePair<int, int>(voter.UserId, owner.VotesReceived[voterIndex].Value + 1);
        else
            owner.VotesReceived.Add(new KeyValuePair<int, int>(voter.UserId, 1));

        var index = voter.VotesGiven.FindIndex(kv => kv.Key == owner.UserId);

        if (index >= 0)
            voter.VotesGiven[index] = new KeyValuePair<int, int>(owner.UserId, voter.VotesGiven[index].Value + 1);
        else
            voter.VotesGiven.Add(new KeyValuePair<int, int>(owner.UserId, 1));
    }

}
