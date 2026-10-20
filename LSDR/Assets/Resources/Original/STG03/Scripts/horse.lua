audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 3.75
leapDistance = 5.5
interacted = false

function start()
    if this.GameObject.Name ~= "Horse1" and IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    this.SnapToFloor()
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    this.LogGraphContribution(-3, 1)
    audio.Play()
    this.PlayAnimation(0)
    interacted = true

    this.Action
        .WaitUntil(Condition.WaitForSeconds(5.5))
        .Then(function()
            this.GameObject.WorldPosition = this.GameObject.WorldPosition + this.Forward * leapDistance
        end)
        .ThenWaitUntil(Condition.WaitForSeconds(40))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end