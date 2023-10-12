updating = false
moveSpeed = 0.2

function start()
    if Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.SetChildVisible(false)
end

function update()
    if not updating then return end

    this.MoveInDirection(this.GameObject.UpDirection, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    updating = true
    DreamSystem.LogGraphContributionFromEntity(3, 3)
end