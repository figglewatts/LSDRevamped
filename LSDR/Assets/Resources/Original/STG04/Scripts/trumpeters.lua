trumpeter1 = GetEntity("Trumpeter1")
trumpeter2 = GetEntity("Trumpeter2")
trumpeter3 = GetEntity("Trumpeter3")
trumpeter4 = GetEntity("Trumpeter4")
trumpeter5 = GetEntity("Trumpeter5")
trumpeter6 = GetEntity("Trumpeter6")
audio = GetEntity("TrumpetersAudio").DreamAudio

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    hideTrumpeters()
end

function interact()
    showTrumpeters()
    audio.Play()
end

function hideTrumpeters()
    trumpeter1.InteractiveObject.SetChildVisible(false)
    trumpeter2.InteractiveObject.SetChildVisible(false)
    trumpeter3.InteractiveObject.SetChildVisible(false)
    trumpeter4.InteractiveObject.SetChildVisible(false)
    trumpeter5.InteractiveObject.SetChildVisible(false)
    trumpeter6.InteractiveObject.SetChildVisible(false)
end

function showTrumpeters()
    trumpeter1.InteractiveObject.SetChildVisible(true)
    trumpeter2.InteractiveObject.SetChildVisible(true)
    trumpeter3.InteractiveObject.SetChildVisible(true)
    trumpeter4.InteractiveObject.SetChildVisible(true)
    trumpeter5.InteractiveObject.SetChildVisible(true)
    trumpeter6.InteractiveObject.SetChildVisible(true)
end