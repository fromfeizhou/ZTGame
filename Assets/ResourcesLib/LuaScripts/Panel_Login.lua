local claine_pb = require 'Protol.claine_pb'       


local pb_data


function init_panel()
	widget:BandingBtn_OnClick(Btn_Login,"ViewPort/Btn_Login")
	widget:BandingBtn_OnClick(Btn_Regedit,"ViewPort/Btn_Regedit")
end

function Btn_Login()
	print("lua -> Btn_Login")
end

function Btn_Regedit()
	print("lua -> Btn_Regedit")
end

function update()
	print("update")
end


function Decoder()  
	local claine = claine_pb.Claine()
	claine:ParseFromString(pb_data)

	print('Claine id:' .. claine.id)
	print('Claine name:' .. claine.name)
	print('Claine age:' .. claine.age)
end

function Encoder()       
	local claine = claine_pb.Claine()
	claine.id = 996
	claine.name = 'lekailin'
	claine.age = 18
	pb_data = claine:SerializeToString()  
end
