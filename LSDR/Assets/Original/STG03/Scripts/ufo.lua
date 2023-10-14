audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 1

function start()
    this.SetChildVisible(false)
end

function interact()
    this.SetChildVisible(true)
    audio.Play()
    DreamSystem.LogGraphContributionFromEntity(8, -1)
    this.PlayAnimation(0)

    this.Action
        .Do(|| this.MoveInDirection(this.Forward, moveSpeed))
        .Until(Condition.WaitForSeconds(20))
        .Then(function() 
            this.MoveInDirection(this.Forward, moveSpeed)
            this.MoveInDirection(this.Up, moveSpeed)
        end)
        .Until(Condition.WaitForSeconds(30))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end