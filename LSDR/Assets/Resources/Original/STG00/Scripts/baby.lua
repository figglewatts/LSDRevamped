require "dreams"

player = GetEntity("__player")
audio = GetEntity("BabyAudio").DreamAudio

fades = false
met = false
linked = false
walkSpeed = 0.2

function start()
    -- only on odd days, and not every third one
    if IsDayEven() or IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    -- some days it's hidden and fades in when you look down at it, otherwise it stands frozen
    fades = IsDayLinear(3, 1)
    if fades then
        this.SetChildVisible(false)
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(1, 0, 0)
    else
        this.PlayAnimation(0)
        this.StopAnimation()
    end
end

function intervalUpdate()
    if fades or met or linked then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.5 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    met = true
    audio.Play()
    SetCanControlPlayer(false)

    if fades then
        fadeIn()
        return
    end

    this.LogGraphContribution(-3, -4)
    this.LookAtPlane(player.WorldPosition)
    this.PlayAnimation(1)
    this.Action
        .WaitUntil(this.WaitForAnimation(1))
        .Then(|| DreamSystem.TransitionToDream())
        .ThenFinish()
end

function fadeIn()
    if Random.OneIn(2) then
        this.GameObject.WorldPosition = this.GameObject.WorldPosition - Unity.Vector3(0, 0.125, 0)
    end

    this.SetChildVisible(true)
    DreamSystem.FadeFromBlack(0.5)
    this.PlayAnimation(0)
    this.LogGraphContribution(0, 1)

    this.Action
        .Do(walkToPlayer)
        .Until(Condition.WaitForSeconds(2.5))
        .Then(|| DreamSystem.TransitionToDream())
        .ThenFinish()
end

function walkToPlayer()
    -- model faces backwards
    local position = this.GameObject.WorldPosition
    this.LookAtPlane(position + (position - player.WorldPosition))
    this.MoveTowards(player.WorldPosition, walkSpeed)
end