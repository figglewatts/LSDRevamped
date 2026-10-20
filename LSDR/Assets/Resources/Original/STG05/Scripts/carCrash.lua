hornAudio = GetEntity("CarHornAudio").DreamAudio
splashAudio = GetEntity("CarSplashAudio").DreamAudio

function start()
    if not IsDayEven() or IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end
end

crashed = false
crashTime = -1

-- hold the last frame once it's played through, so it stays in the water
function update()
    if crashTime < 0 then return end

    crashTime = crashTime + Unity.DeltaTime()
    if crashTime >= this.GetAnimationLengthSeconds(0) - 0.025 then
        this.StopAnimation()
        crashTime = -1
    end
end

function interact()
    this.LogGraphContribution(-5, 5)

    if crashed then return end
    crashed = true

    this.Action
        .Do(|| hornAudio.Play())
        .Then(function()
            this.PlayAnimation(0)
            crashTime = 0
        end)
        .ThenWaitUntil(Condition.WaitForSeconds(3))
        .Then(|| hornAudio.Stop())
        .ThenWaitUntil(Condition.WaitForSeconds(1))
        .Then(|| splashAudio.Play())
        .ThenFinish()
end