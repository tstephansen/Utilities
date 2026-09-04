dotnet publish -c Release -o bin/publish
cp bin/publish/AsteriskFixer bin/publish/afixer
sudo cp bin/publish/afixer /usr/local/bin/