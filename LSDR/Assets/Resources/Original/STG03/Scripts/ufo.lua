audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 1.25
climbSpeed = 0.3

function start()
    if this.GameObject.Name ~= "UFO1" and IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function interact()
    this.SetChildVisible(true)
    audio.Play()
    this.LogGraphContribution(8, -1)
    this.PlayAnimation(0)

    this.Action
        .Do(function()
            this.MoveInDirection(this.Forward, moveSpeed)
            this.MoveInDirection(this.Up, climbSpeed)
        end)
        .Until(Condition.WaitForSeconds(5))
        .Then(|| this.MoveInDirection(this.Forward, moveSpeed))
        .Until(Condition.WaitForSeconds(10))
        .Then(function()
            this.MoveInDirection(this.Forward, moveSpeed)
            this.MoveInDirection(this.Up.negated(), climbSpeed)
        end)
        .Until(Condition.WaitForSeconds(30))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end