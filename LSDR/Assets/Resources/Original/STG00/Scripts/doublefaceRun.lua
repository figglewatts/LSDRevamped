-- sometimes doesn't run
-- delay before running random
-- speed random

require "dreams"

player = GetEntity("__player")
target = GetEntity("DoublefaceTarget")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 0.25
delay = 0
linked = false
distanceToPlayer = 0

function activeOnDay(day)
    local zeroDay = ((day / 2) - 1) % 3
    if this.GameObject.Name == "DoublefaceA" then
        return zeroDay == 0 or zeroDay == 1 or zeroDay == 2
    else
        return zeroDay == 1
    end
end

function start()
    moveSpeed = Random.FloatMinMax(0.4, 0.75)
    delay = Random.FloatMinMax(0, 0.3)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if not activeOnDay(DreamSystem.DayNumber) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if linked then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    if this.GameObject.Name == "DoublefaceA" then
        this.LogGraphContribution(2, 8)
    else
        this.LogGraphContribution(0, 5)
    end

    audio.Play()

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(delay))
        .Then(|| this.PlayAnimation(1))
        .Then(|| this.MoveTowards(target.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target.WorldPosition))
        .ThenFinish()
end